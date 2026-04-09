import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';

// Pipes
import {
  HostStatusPipe,
  ServiceNodeStatusPipe,
  AlertSeverityPipe,
  AlertStatusPipe,
} from './pipes/status.pipe';

const MATERIAL_MODULES = [
  MatButtonModule,
  MatIconModule,
  MatTableModule,
  MatCardModule,
  MatInputModule,
  MatFormFieldModule,
  MatSelectModule,
  MatChipsModule,
  MatProgressSpinnerModule,
  MatSnackBarModule,
  MatDialogModule,
  MatTooltipModule,
  MatBadgeModule,
  MatDividerModule,
];

const PIPES = [
  HostStatusPipe,
  ServiceNodeStatusPipe,
  AlertSeverityPipe,
  AlertStatusPipe,
];

@NgModule({
  declarations: [...PIPES],
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule, ...MATERIAL_MODULES],
  exports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    ...MATERIAL_MODULES,
    ...PIPES,
  ],
})
export class SharedModule {}
