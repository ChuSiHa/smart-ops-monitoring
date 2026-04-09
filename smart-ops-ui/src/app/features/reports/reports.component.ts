import { Component, OnInit } from '@angular/core';
import type { EChartsOption } from 'echarts';
import { HostService } from '../../core/services/host.service';
import { AlertService } from '../../core/services/alert.service';
import { MetricService } from '../../core/services/metric.service';
import { AlertSeverity, HostStatus } from '../../core/models/models';

@Component({
  standalone: false,
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss'],
})
export class ReportsComponent implements OnInit {
  heatmapOption: EChartsOption = {};
  alertBarOption: EChartsOption = {};
  hostPieOption: EChartsOption = {};
  loading = true;

  constructor(
    private hostService: HostService,
    private alertService: AlertService,
    private metricService: MetricService
  ) {}

  ngOnInit(): void {
    Promise.all([
      this.hostService.getAll().toPromise(),
      this.alertService.getAll().toPromise(),
    ]).then(([hosts, alerts]) => {
      hosts = hosts ?? [];
      alerts = alerts ?? [];
      this.buildHostPie(hosts);
      this.buildAlertBar(alerts);
      this.buildHeatmap(alerts);
      this.loading = false;
    });
  }

  private buildHostPie(hosts: any[]): void {
    const byStatus = [0, 0, 0, 0]; // Unknown, Online, Offline, Maintenance
    hosts.forEach((h) => byStatus[h.status]++);
    this.hostPieOption = {
      backgroundColor: 'transparent',
      tooltip: { trigger: 'item', backgroundColor: '#1e2636', borderColor: '#374151', textStyle: { color: '#e2e8f0' } },
      legend: { orient: 'vertical', left: 'left', textStyle: { color: '#94a3b8' } },
      series: [{
        name: 'Hosts',
        type: 'pie',
        radius: ['45%', '72%'],
        avoidLabelOverlap: false,
        itemStyle: { borderRadius: 5, borderColor: '#161b27', borderWidth: 2 },
        label: { show: false },
        emphasis: { label: { show: true, fontSize: 13, fontWeight: 'bold' } },
        data: [
          { value: byStatus[0], name: 'Unknown', itemStyle: { color: '#374151' } },
          { value: byStatus[1], name: 'Online',  itemStyle: { color: '#22c55e' } },
          { value: byStatus[2], name: 'Offline', itemStyle: { color: '#ef4444' } },
          { value: byStatus[3], name: 'Maintenance', itemStyle: { color: '#f59e0b' } },
        ],
      }],
    };
  }

  private buildAlertBar(alerts: any[]): void {
    const days = this.last7Days();
    const data = days.map((d) => ({
      day: d,
      info: alerts.filter((a) => a.severity === AlertSeverity.Info && this.sameDay(a.createdAt, d)).length,
      warning: alerts.filter((a) => a.severity === AlertSeverity.Warning && this.sameDay(a.createdAt, d)).length,
      critical: alerts.filter((a) => a.severity === AlertSeverity.Critical && this.sameDay(a.createdAt, d)).length,
    }));

    this.alertBarOption = {
      backgroundColor: 'transparent',
      tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' }, backgroundColor: '#1e2636', borderColor: '#374151', textStyle: { color: '#e2e8f0' } },
      legend: { textStyle: { color: '#94a3b8' } },
      grid: { left: 40, right: 20, top: 40, bottom: 40 },
      xAxis: {
        type: 'category',
        data: data.map((d) => d.day),
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: '#64748b', fontSize: 11 },
        splitLine: { lineStyle: { color: '#1e2636', type: 'dashed' } },
      },
      series: [
        { name: 'Info', type: 'bar', stack: 'total', itemStyle: { color: '#60a5fa' }, data: data.map((d) => d.info) },
        { name: 'Warning', type: 'bar', stack: 'total', itemStyle: { color: '#f59e0b' }, data: data.map((d) => d.warning) },
        { name: 'Critical', type: 'bar', stack: 'total', itemStyle: { color: '#ef4444' }, data: data.map((d) => d.critical) },
      ],
    };
  }

  private buildHeatmap(alerts: any[]): void {
    const hours = Array.from({ length: 24 }, (_, i) => `${i}:00`);
    const days = this.last7Days();

    const heatData: [number, number, number][] = [];
    days.forEach((day, di) => {
      hours.forEach((_, hi) => {
        const count = alerts.filter((a) => {
          const d = new Date(a.createdAt);
          return this.sameDay(a.createdAt, day) && d.getHours() === hi;
        }).length;
        heatData.push([hi, di, count]);
      });
    });

    const maxVal = Math.max(...heatData.map((d) => d[2]), 1);

    this.heatmapOption = {
      backgroundColor: 'transparent',
      tooltip: {
        position: 'top',
        backgroundColor: '#1e2636',
        borderColor: '#374151',
        textStyle: { color: '#e2e8f0' },
        formatter: (params: any) => `${days[params.data[1]]} ${hours[params.data[0]]}: ${params.data[2]} alerts`,
      },
      grid: { left: 70, right: 20, top: 20, bottom: 60 },
      xAxis: {
        type: 'category',
        data: hours,
        axisLabel: { color: '#64748b', fontSize: 10 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitArea: { show: true, areaStyle: { color: ['#0f1117', '#161b27'] } },
      },
      yAxis: {
        type: 'category',
        data: days,
        axisLabel: { color: '#64748b', fontSize: 11 },
        axisLine: { lineStyle: { color: '#1e2636' } },
        splitArea: { show: true, areaStyle: { color: ['#0f1117', '#161b27'] } },
      },
      visualMap: {
        min: 0,
        max: maxVal,
        calculable: true,
        orient: 'horizontal',
        left: 'center',
        bottom: 0,
        textStyle: { color: '#94a3b8' },
        inRange: { color: ['#1e2636', '#1e3a5f', '#1d4ed8', '#60a5fa', '#ef4444'] },
      },
      series: [{
        name: 'Alert Frequency',
        type: 'heatmap',
        data: heatData,
        label: { show: false },
        emphasis: { itemStyle: { shadowBlur: 10, shadowColor: 'rgba(0, 0, 0, 0.5)' } },
      }],
    };
  }

  private last7Days(): string[] {
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date();
      d.setDate(d.getDate() - (6 - i));
      return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    });
  }

  private sameDay(dateStr: string, label: string): boolean {
    const d = new Date(dateStr);
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }) === label;
  }
}
