import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AlertDto, CreateAlertCommand, UpdateAlertStatusCommand, GetAlertsParams } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AlertService {
  private readonly base = '/api/alerts';

  constructor(private http: HttpClient) {}

  getAll(filter?: GetAlertsParams): Observable<AlertDto[]> {
    let params = new HttpParams();
    if (filter?.hostId) params = params.set('hostId', filter.hostId);
    if (filter?.status) params = params.set('status', filter.status);
    if (filter?.severity) params = params.set('severity', filter.severity);
    return this.http.get<AlertDto[]>(this.base, { params });
  }

  create(command: CreateAlertCommand): Observable<AlertDto> {
    return this.http.post<AlertDto>(this.base, command);
  }

  updateStatus(id: string, command: UpdateAlertStatusCommand): Observable<AlertDto> {
    return this.http.patch<AlertDto>(`${this.base}/${id}/status`, command);
  }
}
