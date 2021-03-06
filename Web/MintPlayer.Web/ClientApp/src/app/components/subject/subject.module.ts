import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SubjectLikeComponent } from './subject-like/subject-like.component';
import { MediaManagerComponent } from './media-manager/media-manager.component';
import { MediaListComponent } from './media-list/media-list.component';

@NgModule({
  declarations: [
    SubjectLikeComponent,
    MediaManagerComponent,
    MediaListComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    SubjectLikeComponent,
    MediaManagerComponent,
    MediaListComponent
  ]
})
export class SubjectModule { }
