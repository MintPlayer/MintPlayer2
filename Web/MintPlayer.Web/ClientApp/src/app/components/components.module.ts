import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SubjectModule } from './subject/subject.module';



@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SubjectModule
  ],
  declarations: [
    SidebarComponent
  ],
  exports: [
    SidebarComponent,
    SubjectModule
  ]
})
export class ComponentsModule { }
