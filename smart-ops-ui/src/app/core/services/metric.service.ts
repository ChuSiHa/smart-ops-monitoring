import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MetricDto, IngestMetricCommand, GetMetricsParams } from '../models/models';

@Injectable({ providedIn: 'root' })
export class MetricService {
  private readonly base = '/api/metrics';

  constructor(private http: HttpClient) {}

  ingest(command: IngestMetricCommand): Observable<MetricDto> {
    return this.http.post<MetricDto>(`${this.base}/ingest`, command);
  }

  getByHost(hostId: string, filter?: GetMetricsParams): Observable<MetricDto[]> {
    let params = new HttpParams();
    if (filter?.metricType) params = params.set('metricType', filter.metricType);
    if (filter?.from) params = params.set('from', filter.from);
    if (filter?.to) params = params.set('to', filter.to);
    return this.http.get<MetricDto[]>(`${this.base}/host/${hostId}`, { params });
  }
}
