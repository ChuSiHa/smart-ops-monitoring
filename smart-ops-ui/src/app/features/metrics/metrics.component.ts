import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import type { EChartsOption } from 'echarts';

import { MetricService } from '../../core/services/metric.service';
import { HostService } from '../../core/services/host.service';
import { SignalRService } from '../../core/services/signalr.service';
import { MetricDto, HostDto } from '../../core/models/models';

@Component({
  selector: 'app-metrics',
  templateUrl: './metrics.component.html',
  styleUrls: ['./metrics.component.scss'],
})
export class MetricsComponent implements OnInit {
  hosts: HostDto[] = [];
  metrics: MetricDto[] = [];
  selectedHostId = '';
  filterType = '';
  loading = false;
  showIngestForm = false;

  lineChartOption: EChartsOption = {};

  ingestForm: FormGroup;

  constructor(
    private metricService: MetricService,
    private hostService: HostService,
    private signalRService: SignalRService,
    private snackBar: MatSnackBar,
    private fb: FormBuilder
  ) {
    this.ingestForm = this.fb.group({
      hostId: ['', Validators.required],
      metricType: ['', Validators.required],
      value: [0, [Validators.required, Validators.min(0)]],
      unit: [''],
    });
  }

  ngOnInit(): void {
    this.hostService.getAll().subscribe((hosts) => {
      this.hosts = hosts;
      if (hosts.length) {
        this.selectedHostId = hosts[0].id;
        this.loadMetrics();
        this.signalRService.startMetricHub(hosts[0].id);
      }
    });

    // Append live metrics to chart
    this.signalRService.metricReceived$.subscribe((m) => {
      if (m.hostId === this.selectedHostId) {
        this.metrics = [...this.metrics, m].slice(-120);
        this.buildChart();
      }
    });
  }

  loadMetrics(): void {
    if (!this.selectedHostId) return;
    this.loading = true;
    this.metricService
      .getByHost(this.selectedHostId, this.filterType ? { metricType: this.filterType } : {})
      .subscribe({
        next: (m) => {
          this.metrics = m.slice(-120);
          this.buildChart();
          this.loading = false;
        },
        error: () => { this.loading = false; },
      });
  }

  onHostChange(): void {
    this.loadMetrics();
  }

  ingest(): void {
    if (this.ingestForm.invalid) return;
    const v = this.ingestForm.value;
    this.metricService
      .ingest({
        hostId: v.hostId!,
        metricType: v.metricType!,
        value: v.value!,
        unit: v.unit ?? '',
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Metric ingested', 'Dismiss', { duration: 2000 });
          if (v.hostId === this.selectedHostId) this.loadMetrics();
        },
      });
  }

  private buildChart(): void {
    const grouped = this.metrics.reduce<Record<string, MetricDto[]>>((acc, m) => {
      acc[m.metricType] = acc[m.metricType] ?? [];
      acc[m.metricType].push(m);
      return acc;
    }, {});

    const colors = ['#60a5fa', '#a78bfa', '#34d399', '#fb923c', '#f472b6', '#facc15'];
    const series = Object.entries(grouped).map(([type, data], i) => ({
      name: type,
      type: 'line' as const,
      smooth: true,
      symbol: 'none',
      itemStyle: { color: colors[i % colors.length] },
      areaStyle: { opacity: 0.08 },
      lineStyle: { width: 2 },
      data: data.map((m) => [m.timestamp, m.value]),
    }));

    this.lineChartOption = {
      backgroundColor: 'transparent',
      tooltip: {
        trigger: 'axis',
        backgroundColor: '#1e2636',
        borderColor: '#374151',
        textStyle: { color: '#e2e8f0' },
        axisPointer: { type: 'cross', label: { backgroundColor: '#1e2636' } },
      },
      legend: { textStyle: { color: '#94a3b8' }, top: 4 },
      toolbox: {
        feature: {
          dataZoom: { yAxisIndex: 'none' },
          restore: {},
          saveAsImage: {},
        },
        iconStyle: { borderColor: '#64748b' },
      },
      grid: { left: 55, right: 24, top: 50, bottom: 60 },
      xAxis: {
        type: 'time',
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitLine: { lineStyle: { color: '#1e2636', type: 'dashed' } },
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitLine: { lineStyle: { color: '#1e2636', type: 'dashed' } },
      },
      dataZoom: [
        { type: 'inside', start: 0, end: 100 },
        { start: 0, end: 100, handleStyle: { color: '#374151' }, textStyle: { color: '#94a3b8' }, borderColor: '#374151', fillerColor: 'rgba(96,165,250,0.1)' },
      ],
      series: series.length ? series : [{ name: 'No Data', type: 'line', data: [] }],
    };
  }
}
