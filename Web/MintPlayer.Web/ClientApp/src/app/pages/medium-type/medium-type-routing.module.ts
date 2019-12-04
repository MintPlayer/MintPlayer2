import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MediumTypeListComponent } from './list/list.component';
import { MediumTypeCreateComponent } from './create/create.component';
import { MediumTypeEditComponent } from './edit/edit.component';
import { MediumTypeShowComponent } from './show/show.component';
import { IsLoggedInGuard } from '../../guards/is-logged-in/is-logged-in.guard';

const routes: Routes = [
  { path: '', component: MediumTypeListComponent },
  { path: 'create', component: MediumTypeCreateComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id/edit', component: MediumTypeEditComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id', component: MediumTypeShowComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MediumTypeRoutingModule { }
