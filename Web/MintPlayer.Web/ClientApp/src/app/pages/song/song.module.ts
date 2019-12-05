import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { SongRoutingModule } from './song-routing.module';
import { SongListComponent } from './list/list.component';
import { SongCreateComponent } from './create/create.component';
import { SongEditComponent } from './edit/edit.component';
import { SongShowComponent } from './show/show.component';
import { ControlsModule } from '../../controls/controls.module';
import { ComponentsModule } from '../../components/components.module';


@NgModule({
  declarations: [
    SongListComponent,
    SongCreateComponent,
    SongEditComponent,
    SongShowComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ControlsModule,
    ComponentsModule,
    SongRoutingModule
  ]
})
export class SongModule { }
