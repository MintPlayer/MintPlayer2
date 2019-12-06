import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SubjectModule } from './subject/subject.module';
import { YoutubePlayerComponent } from './youtube-player/youtube-player.component';
import { YoutubePlayButtonComponent } from './youtube-play-button/youtube-play-button.component';
import { PlaylistSidebarComponent } from './playlist-sidebar/playlist-sidebar.component';



@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SubjectModule
  ],
  declarations: [
    SidebarComponent,
    YoutubePlayerComponent,
    YoutubePlayButtonComponent,
    PlaylistSidebarComponent
  ],
  exports: [
    SidebarComponent,
    SubjectModule,
    YoutubePlayerComponent,
    YoutubePlayButtonComponent,
    PlaylistSidebarComponent
  ]
})
export class ComponentsModule { }
