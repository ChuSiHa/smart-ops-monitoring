import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { AlertsComponent } from './alerts.component';

const routes: Routes = [{ path: '', component: AlertsComponent }];

@NgModule({
  declarations: [AlertsComponent],
  imports: [SharedModule, RouterModule.forChild(routes)],
})
export class AlertsModule {}
