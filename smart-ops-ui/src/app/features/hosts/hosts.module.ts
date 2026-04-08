import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { HostsComponent } from './hosts.component';
import { HostDetailComponent } from './host-detail/host-detail.component';

const routes: Routes = [
  { path: '', component: HostsComponent },
  { path: ':id', component: HostDetailComponent },
];

@NgModule({
  declarations: [HostsComponent, HostDetailComponent],
  imports: [SharedModule, RouterModule.forChild(routes)],
})
export class HostsModule {}
