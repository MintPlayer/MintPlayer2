import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ArtistRoutingModule } from './artist-routing.module';
import { ArtistListComponent } from './list/list.component';
import { ArtistCreateComponent } from './create/create.component';
import { ArtistEditComponent } from './edit/edit.component';
import { ArtistShowComponent } from './show/show.component';
import { ControlsModule } from '../../controls/controls.module';
import { ComponentsModule } from '../../components/components.module';


@NgModule({
  declarations: [
    ArtistListComponent,
    ArtistCreateComponent,
    ArtistEditComponent,
    ArtistShowComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ControlsModule,
    ComponentsModule,
    ArtistRoutingModule
  ]
})
export class ArtistModule { }
