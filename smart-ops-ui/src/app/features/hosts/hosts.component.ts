import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HostService } from '../../core/services/host.service';
import { HostDto, HostStatus } from '../../core/models/models';

@Component({
  selector: 'app-hosts',
  templateUrl: './hosts.component.html',
  styleUrls: ['./hosts.component.scss'],
})
export class HostsComponent implements OnInit {
  displayedColumns = ['name', 'ip', 'osType', 'status', 'tags', 'actions'];
  hosts: HostDto[] = [];
  loading = true;
  showCreateForm = false;
  HostStatus = HostStatus;

  createForm: FormGroup;

  constructor(
    private hostService: HostService,
    private snackBar: MatSnackBar,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      name: ['', Validators.required],
      ipAddress: ['', Validators.required],
      osType: ['', Validators.required],
      tags: [''],
    });
  }

  ngOnInit(): void {
    this.loadHosts();
  }

  loadHosts(): void {
    this.loading = true;
    this.hostService.getAll().subscribe({
      next: (hosts) => { this.hosts = hosts; this.loading = false; },
      error: () => { this.loading = false; },
    });
  }

  createHost(): void {
    if (this.createForm.invalid) return;
    const v = this.createForm.value;
    const tags = v.tags ? v.tags.split(',').map((t: string) => t.trim()).filter(Boolean) : [];
    this.hostService
      .create({ name: v.name!, ipAddress: v.ipAddress!, osType: v.osType!, tags })
      .subscribe({
        next: (h) => {
          this.hosts = [h, ...this.hosts];
          this.createForm.reset();
          this.showCreateForm = false;
          this.snackBar.open('Host created', 'Dismiss', { duration: 3000 });
        },
      });
  }
}
