import { Component, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { eToggleButtonState } from './enums/eToggleButtonState';
import { eSidebarState } from './enums/eSidebarState';
import { User } from './interfaces/account/user';
import { LoginComponent } from './pages/account/login/login.component';
import { RegisterComponent } from './pages/account/register/register.component';
import { AccountService } from './services/account/account.service';
import { YoutubeHelper } from './helpers/youtube-helper';
import { Song } from './interfaces/song';
import { SongShowComponent } from './pages/song/show/show.component';
import { YoutubePlayerComponent } from './components/youtube-player/youtube-player.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'ClientApp';
  activeUser: User = null;
  fullWidth: boolean = false;
  toggleButtonState: eToggleButtonState = eToggleButtonState.auto;
  sidebarState: eSidebarState = eSidebarState.auto;
  playlistToggleButtonState: eToggleButtonState = eToggleButtonState.closed;

  constructor(private accountService: AccountService, private youtubeHelper: YoutubeHelper, private ref: ChangeDetectorRef) {
    this.accountService.currentUser().then((user) => {
      this.activeUser = user;
    }).catch((error) => {
      this.activeUser = null;
    });
    this.youtubeHelper.loadApi().then(() => {
      console.log('loaded youtube api');
      this.youtubeHelper.apiReady.next(true);
    });
  }

  updateSidebarState(state: eToggleButtonState) {
    switch (state) {
      case eToggleButtonState.open:
        this.sidebarState = eSidebarState.show;
        break;
      case eToggleButtonState.closed:
        this.sidebarState = eSidebarState.hide;
        break;
      default:
        this.sidebarState = eSidebarState.auto;
        break;
    }
  }

  collapseSidebar() {
    if (window.innerWidth < 768) {
      this.toggleButtonState = eToggleButtonState.closed;
      this.sidebarState = eSidebarState.hide;
    }
  }

  loginCompleted = (user: User) => {
    this.activeUser = user;
  }

  logoutClicked() {
    this.accountService.logout().then(() => {
      this.activeUser = null;
    });
  }

  //#region Playlist
  playlist: Song[] = [];
  currentsong: Song;
  endOfPlaylist: boolean = true;
  addToPlaylist = (song: Song) => {
    this.playlist.push(song);
    if (this.endOfPlaylist) {
      this.playSong(song);
    }
  }

  @ViewChild('player', { static: false }) player: YoutubePlayerComponent;

  private playSong(song: Song) {
    if (song.youtubeId !== null) {
      this.endOfPlaylist = false;
      this.currentsong = song;
      this.player.playSong(song.youtubeId);
    }
  }

  songEnded() {
    var current_index = this.playlist.indexOf(this.currentsong);
    if (this.playlist.length > current_index + 1) {
      var song = this.playlist[current_index + 1];
      this.playSong(song);
    } else {
      this.endOfPlaylist = true;
      this.currentsong = null;
      this.ref.detectChanges();
    }
  }
  //#endregion

  routingActivated(element: ElementRef) {
    // Login complete
    if (element instanceof LoginComponent) {
      element.loginComplete.subscribe(this.loginCompleted);
    } else if (element instanceof RegisterComponent) {
      element.loginComplete.subscribe(this.loginCompleted);
    } else if (element instanceof SongShowComponent) {
      element.addToPlaylist.subscribe(this.addToPlaylist);
    }
  }

  routingDeactivated(element: ElementRef) {
    // Login complete
    if (element instanceof LoginComponent) {
      element.loginComplete.unsubscribe();
    } else if (element instanceof RegisterComponent) {
      element.loginComplete.unsubscribe();
    } else if (element instanceof SongShowComponent) {
      element.addToPlaylist.unsubscribe();
    }
  }
}
