import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PersonListComponent } from './list/list.component';
import { PersonCreateComponent } from './create/create.component';
import { PersonEditComponent } from './edit/edit.component';
import { PersonShowComponent } from './show/show.component';
import { IsLoggedInGuard } from '../../guards/is-logged-in/is-logged-in.guard';

const routes: Routes = [
  { path: '', component: PersonListComponent },
  { path: 'create', component: PersonCreateComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id/edit', component: PersonEditComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id', component: PersonShowComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PersonRoutingModule { }
