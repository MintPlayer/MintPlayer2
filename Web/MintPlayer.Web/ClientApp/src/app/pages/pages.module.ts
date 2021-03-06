import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PagesRoutingModule } from './pages-routing.module';
import { HomeComponent } from './home/home.component';
import { AccountModule } from './account/account.module';
import { PersonModule } from './person/person.module';
import { ArtistModule } from './artist/artist.module';
import { SongModule } from './song/song.module';
import { MediumTypeModule } from './medium-type/medium-type.module';
import { SearchComponent } from './search/search.component';
import { ControlsModule } from '../controls/controls.module';
import { NotFoundComponent } from './not-found/not-found.component';


@NgModule({
  declarations: [HomeComponent, SearchComponent, NotFoundComponent],
  imports: [
    CommonModule,
    ControlsModule,
    PagesRoutingModule,
    AccountModule,
    PersonModule,
    ArtistModule,
    SongModule,
    MediumTypeModule
  ]
})
export class PagesModule { }
