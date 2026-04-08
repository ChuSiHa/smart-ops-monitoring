import { Component, OnInit, OnDestroy } from '@angular/core';
import { combineLatest, interval, merge, Subject, BehaviorSubject } from 'rxjs';
import { switchMap, takeUntil, tap, startWith } from 'rxjs/operators';
import type { EChartsOption } from 'echarts';

import { HostService } from '../../core/services/host.service';
import { AlertService } from '../../core/services/alert.service';
import { MetricService } from '../../core/services/metric.service';
import { SignalRService } from '../../core/services/signalr.service';
import {
  HostDto,
  AlertDto,
  MetricDto,
  HostStatus,
  AlertSeverity,
  AlertStatus,
} from '../../core/models/models';

@Component({
  standalone: false,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  hosts: HostDto[] = [];
  openAlerts: AlertDto[] = [];
  liveMetrics: MetricDto[] = [];
  loading = true;

  // Summary counts
  onlineCount = 0;
  offlineCount = 0;
  openAlertCount = 0;
  criticalCount = 0;

  // ECharts options
  cpuGaugeOption: EChartsOption = {};
  memGaugeOption: EChartsOption = {};
  metricLineOption: EChartsOption = {};
  alertPieOption: EChartsOption = {};

  // Reactive streams for chart data
  private metricBuffer$ = new BehaviorSubject<MetricDto[]>([]);

  constructor(
    private hostService: HostService,
    private alertService: AlertService,
    private metricService: MetricService,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    // Poll hosts & alerts every 30s, merge with SignalR events
    const hostPoll$ = interval(30000).pipe(
      startWith(0),
      switchMap(() => this.hostService.getAll())
    );

    const alertPoll$ = interval(30000).pipe(
      startWith(0),
      switchMap(() => this.alertService.getAll({ status: 'Open' }))
    );

    combineLatest([hostPoll$, alertPoll$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([hosts, alerts]) => {
        this.hosts = hosts;
        this.openAlerts = alerts;
        this.computeSummary();
        this.buildAlertPieChart();
        this.loading = false;
      });

    // Live metrics via SignalR — subscribe to first online host
    merge(this.signalRService.metricReceived$)
      .pipe(takeUntil(this.destroy$))
      .subscribe((metric) => {
        this.liveMetrics = [metric, ...this.liveMetrics].slice(0, 60);
        this.metricBuffer$.next(this.liveMetrics);
        this.updateGauges(metric);
        this.buildMetricLineChart();
      });

    // Live alerts via SignalR
    this.signalRService.alertCreated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((alert) => {
        this.openAlerts = [alert, ...this.openAlerts].slice(0, 50);
        this.computeSummary();
        this.buildAlertPieChart();
      });

    // Start SignalR if we have a host
    this.hostService.getAll().subscribe((hosts) => {
      const firstOnline = hosts.find((h) => h.status === HostStatus.Online);
      if (firstOnline) {
        this.signalRService.startMetricHub(firstOnline.id);
        this.signalRService.startAlertHub(firstOnline.id);
        // Load initial metrics
        this.metricService
          .getByHost(firstOnline.id)
          .subscribe((metrics) => {
            this.liveMetrics = metrics.slice(-60);
            this.metricBuffer$.next(this.liveMetrics);
            this.buildMetricLineChart();
            this.buildInitialGauges(metrics);
          });
      }
    });

    this.buildEmptyCharts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private computeSummary(): void {
    this.onlineCount = this.hosts.filter((h) => h.status === HostStatus.Online).length;
    this.offlineCount = this.hosts.filter((h) => h.status === HostStatus.Offline).length;
    this.openAlertCount = this.openAlerts.filter((a) => a.status === AlertStatus.Open).length;
    this.criticalCount = this.openAlerts.filter(
      (a) => a.severity === AlertSeverity.Critical && a.status === AlertStatus.Open
    ).length;
  }

  private buildEmptyCharts(): void {
    this.cpuGaugeOption = this.makeGauge('CPU', 0, '#60a5fa');
    this.memGaugeOption = this.makeGauge('Memory', 0, '#a78bfa');
    this.buildMetricLineChart();
    this.buildAlertPieChart();
  }

  private buildInitialGauges(metrics: MetricDto[]): void {
    const cpuMetrics = metrics.filter((m) => m.metricType.toLowerCase().includes('cpu'));
    const memMetrics = metrics.filter((m) => m.metricType.toLowerCase().includes('mem'));
    const lastCpu = cpuMetrics.at(-1)?.value ?? 0;
    const lastMem = memMetrics.at(-1)?.value ?? 0;
    this.cpuGaugeOption = this.makeGauge('CPU %', Math.min(lastCpu, 100), '#60a5fa');
    this.memGaugeOption = this.makeGauge('Memory %', Math.min(lastMem, 100), '#a78bfa');
  }

  private updateGauges(metric: MetricDto): void {
    const t = metric.metricType.toLowerCase();
    if (t.includes('cpu')) {
      this.cpuGaugeOption = this.makeGauge('CPU %', Math.min(metric.value, 100), '#60a5fa');
    } else if (t.includes('mem')) {
      this.memGaugeOption = this.makeGauge('Memory %', Math.min(metric.value, 100), '#a78bfa');
    }
  }

  private makeGauge(name: string, value: number, color: string): EChartsOption {
    return {
      backgroundColor: 'transparent',
      series: [
        {
          type: 'gauge',
          startAngle: 200,
          endAngle: -20,
          min: 0,
          max: 100,
          radius: '90%',
          axisLine: {
            lineStyle: {
              width: 14,
              color: [
                [0.3, '#22c55e'],
                [0.7, '#f59e0b'],
                [1, '#ef4444'],
              ],
            },
          },
          pointer: { itemStyle: { color: 'auto' } },
          axisTick: { show: false },
          splitLine: { show: false },
          axisLabel: { show: false },
          detail: {
            valueAnimation: true,
            formatter: '{value}%',
            fontSize: 18,
            fontWeight: 'bold',
            color: color,
            offsetCenter: [0, '65%'],
          },
          title: {
            offsetCenter: [0, '88%'],
            fontSize: 12,
            color: '#94a3b8',
          },
          data: [{ value: Math.round(value), name }],
        },
      ],
    };
  }

  private buildMetricLineChart(): void {
    const metrics = [...this.liveMetrics].reverse();
    const grouped = metrics.reduce<Record<string, MetricDto[]>>((acc, m) => {
      acc[m.metricType] = acc[m.metricType] ?? [];
      acc[m.metricType].push(m);
      return acc;
    }, {});

    const colors = ['#60a5fa', '#a78bfa', '#34d399', '#fb923c', '#f472b6'];
    const series = Object.entries(grouped).map(([type, data], i) => ({
      name: type,
      type: 'line' as const,
      smooth: true,
      symbol: 'none',
      itemStyle: { color: colors[i % colors.length] },
      lineStyle: { width: 2 },
      data: data.map((m) => [m.timestamp, m.value]),
    }));

    this.metricLineOption = {
      backgroundColor: 'transparent',
      tooltip: { trigger: 'axis', backgroundColor: '#1e2636', borderColor: '#374151', textStyle: { color: '#e2e8f0' } },
      legend: { textStyle: { color: '#94a3b8' }, top: 0 },
      grid: { left: 50, right: 20, top: 40, bottom: 40 },
      xAxis: {
        type: 'time',
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitLine: { lineStyle: { color: '#1e2636' } },
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitLine: { lineStyle: { color: '#1e2636' } },
      },
      series: series.length ? series : [{ name: 'No Data', type: 'line', data: [] }],
    };
  }

  private buildAlertPieChart(): void {
    const info = this.openAlerts.filter((a) => a.severity === AlertSeverity.Info).length;
    const warning = this.openAlerts.filter((a) => a.severity === AlertSeverity.Warning).length;
    const critical = this.openAlerts.filter((a) => a.severity === AlertSeverity.Critical).length;

    this.alertPieOption = {
      backgroundColor: 'transparent',
      tooltip: { trigger: 'item', backgroundColor: '#1e2636', borderColor: '#374151', textStyle: { color: '#e2e8f0' } },
      legend: { orient: 'vertical', left: 'left', textStyle: { color: '#94a3b8' } },
      series: [
        {
          name: 'Alerts',
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          itemStyle: { borderRadius: 6, borderColor: '#161b27', borderWidth: 2 },
          label: { show: false },
          emphasis: { label: { show: true, fontSize: 14, fontWeight: 'bold' } },
          data: [
            { value: info, name: 'Info', itemStyle: { color: '#60a5fa' } },
            { value: warning, name: 'Warning', itemStyle: { color: '#f59e0b' } },
            { value: critical, name: 'Critical', itemStyle: { color: '#ef4444' } },
          ],
        },
      ],
    };
  }
}
