import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { combineLatest } from 'rxjs';

import { HostService } from '../../../core/services/host.service';
import { ServiceNodeService } from '../../../core/services/service-node.service';
import { HostDto, ServiceNodeDto, ServiceNodeStatus } from '../../../core/models/models';

@Component({
  selector: 'app-host-detail',
  templateUrl: './host-detail.component.html',
  styleUrls: ['./host-detail.component.scss'],
})
export class HostDetailComponent implements OnInit {
  host: HostDto | null = null;
  serviceNodes: ServiceNodeDto[] = [];
  loading = true;
  showAddNode = false;
  ServiceNodeStatus = ServiceNodeStatus;
  nodeColumns = ['name', 'type', 'port', 'status', 'createdAt'];

  nodeForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private hostService: HostService,
    private serviceNodeService: ServiceNodeService,
    private snackBar: MatSnackBar,
    private fb: FormBuilder
  ) {
    this.nodeForm = this.fb.group({
      name: ['', Validators.required],
      type: ['', Validators.required],
      port: [null as number | null],
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    combineLatest([
      this.hostService.getById(id),
      this.serviceNodeService.getByHost(id),
    ]).subscribe({
      next: ([host, nodes]) => {
        this.host = host;
        this.serviceNodes = nodes;
        this.loading = false;
      },
      error: () => { this.loading = false; },
    });
  }

  addNode(): void {
    if (this.nodeForm.invalid || !this.host) return;
    const v = this.nodeForm.value;
    this.serviceNodeService
      .create({
        name: v.name!,
        type: v.type!,
        hostId: this.host.id,
        port: v.port ?? null,
      })
      .subscribe({
        next: (node) => {
          this.serviceNodes = [...this.serviceNodes, node];
          this.nodeForm.reset();
          this.showAddNode = false;
          this.snackBar.open('Service node added', 'Dismiss', { duration: 3000 });
        },
      });
  }
}
