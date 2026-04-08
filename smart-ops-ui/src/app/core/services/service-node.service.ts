import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceNodeDto, CreateServiceNodeCommand } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ServiceNodeService {
  private readonly base = '/api/servicenodes';

  constructor(private http: HttpClient) {}

  getByHost(hostId: string): Observable<ServiceNodeDto[]> {
    const params = new HttpParams().set('hostId', hostId);
    return this.http.get<ServiceNodeDto[]>(this.base, { params });
  }

  create(command: CreateServiceNodeCommand): Observable<ServiceNodeDto> {
    return this.http.post<ServiceNodeDto>(this.base, command);
  }
}
