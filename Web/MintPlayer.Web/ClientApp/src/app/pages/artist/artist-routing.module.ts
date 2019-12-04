import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ArtistListComponent } from './list/list.component';
import { ArtistCreateComponent } from './create/create.component';
import { ArtistEditComponent } from './edit/edit.component';
import { ArtistShowComponent } from './show/show.component';
import { IsLoggedInGuard } from '../../guards/is-logged-in/is-logged-in.guard';

const routes: Routes = [
  { path: '', component: ArtistListComponent },
  { path: 'create', component: ArtistCreateComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id/edit', component: ArtistEditComponent, canActivate: [IsLoggedInGuard] },
  { path: ':id', component: ArtistShowComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ArtistRoutingModule { }
