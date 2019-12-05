import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MediumTypeRoutingModule } from './medium-type-routing.module';
import { MediumTypeListComponent } from './list/list.component';
import { MediumTypeCreateComponent } from './create/create.component';
import { MediumTypeEditComponent } from './edit/edit.component';
import { MediumTypeShowComponent } from './show/show.component';
import { ControlsModule } from '../../controls/controls.module';


@NgModule({
  declarations: [
    MediumTypeListComponent,
    MediumTypeCreateComponent,
    MediumTypeEditComponent,
    MediumTypeShowComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ControlsModule,
    MediumTypeRoutingModule
  ]
})
export class MediumTypeModule { }
