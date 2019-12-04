import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SongListComponent } from './list/list.component';
import { SongCreateComponent } from './create/create.component';
import { SongEditComponent } from './edit/edit.component';
import { SongShowComponent } from './show/show.component';
import { IsLoggedInGuard } from '../../guards/is-logged-in/is-logged-in.guard';

const routes: Routes = [
  { path: '', component: SongListComponent },
  { path: 'create', component: SongCreateComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id/edit', component: SongEditComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id', component: SongShowComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SongRoutingModule { }
