import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SongRoutingModule } from './song-routing.module';
import { SongListComponent } from './list/list.component';
import { SongCreateComponent } from './create/create.component';
import { SongEditComponent } from './edit/edit.component';
import { SongShowComponent } from './show/show.component';


@NgModule({
  declarations: [
    SongListComponent,
    SongCreateComponent,
    SongEditComponent,
    SongShowComponent
  ],
  imports: [
    CommonModule,
    SongRoutingModule
  ]
})
export class SongModule { }
