import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShellComponent } from './shared/components/shell/shell.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.module').then((m) => m.DashboardModule),
      },
      {
        // All authenticated users (Operators & Admins) may manage hosts.
        path: 'hosts',
        loadChildren: () =>
          import('./features/hosts/hosts.module').then((m) => m.HostsModule),
      },
      {
        path: 'metrics',
        loadChildren: () =>
          import('./features/metrics/metrics.module').then((m) => m.MetricsModule),
      },
      {
        path: 'alerts',
        loadChildren: () =>
          import('./features/alerts/alerts.module').then((m) => m.AlertsModule),
      },
      {
        // All authenticated users may view reports (read-only dashboards).
        path: 'reports',
        loadChildren: () =>
          import('./features/reports/reports.module').then((m) => m.ReportsModule),
      },
      {
        // System settings are restricted to the Admin role.
        path: 'settings',
        canActivate: [RoleGuard],
        data: { roles: ['Admin'] },
        loadChildren: () =>
          import('./features/settings/settings.module').then((m) => m.SettingsModule),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

