import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { PersonRoutingModule } from './person-routing.module';
import { PersonListComponent } from './list/list.component';
import { PersonCreateComponent } from './create/create.component';
import { PersonEditComponent } from './edit/edit.component';
import { PersonShowComponent } from './show/show.component';
import { ControlsModule } from '../../controls/controls.module';
import { ComponentsModule } from '../../components/components.module';


@NgModule({
  declarations: [
    PersonListComponent,
    PersonCreateComponent,
    PersonEditComponent,
    PersonShowComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ControlsModule,
    ComponentsModule,
    PersonRoutingModule
  ]
})
export class PersonModule { }
