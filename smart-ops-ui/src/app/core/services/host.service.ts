import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HostDto, CreateHostCommand } from '../models/models';

@Injectable({ providedIn: 'root' })
export class HostService {
  private readonly base = '/api/hosts';

  constructor(private http: HttpClient) {}

  getAll(): Observable<HostDto[]> {
    return this.http.get<HostDto[]>(this.base);
  }

  getById(id: string): Observable<HostDto> {
    return this.http.get<HostDto>(`${this.base}/${id}`);
  }

  create(command: CreateHostCommand): Observable<HostDto> {
    return this.http.post<HostDto>(this.base, command);
  }
}
