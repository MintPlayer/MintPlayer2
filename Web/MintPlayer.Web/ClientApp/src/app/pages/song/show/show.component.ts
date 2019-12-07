import { Component, OnInit, Output, EventEmitter, OnDestroy, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { Song } from '../../../interfaces/song';
import { SongService } from '../../../services/song/song.service';
import { APP_BASE_HREF, Location } from '@angular/common';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.scss']
})
export class SongShowComponent implements OnInit, OnDestroy {
  constructor(
    private songService: SongService,
    private router: Router,
    private route: ActivatedRoute,
    @Inject('BASE_URL') private baseUrl: string,
    private location: Location,
    private titleService: Title,
    private metaService: Meta) {

    var id = parseInt(this.route.snapshot.paramMap.get("id"));
    this.loadSong(id);
  }

  private loadSong(id: number) {
    return this.songService.getSong(id, true).then(song => {
      this.song = song;
      if (song != null) {
        this.titleService.setTitle(`${song.title}: Video and lyrics`);
      }
    });
  }

  private metaTags: HTMLMetaElement[] = [];
  private addDefaultMeta() {
    if (this.song.youtubeId === null) {
      return this.metaService.addTags([
        { name: 'description', content: `Lyrics of ${this.song.description}` }
      ], true);
    } else {
      return this.metaService.addTags([
        { name: 'description', content: `Video and lyrics of ${this.song.description}` }
      ], true);
    }
  }
  private addTwitterCard() {
    if (this.song.youtubeId === null) {
      return this.metaService.addTags([
        { name: 'twitter:card', content: 'summary_large_image' },
        { name: 'twitter:url', content: `${this.baseUrl}/${this.location.path()}` },
        { name: 'twitter:type', content: 'music.song' },
        { name: 'twitter:title', content: this.song.description },
        { name: 'twitter:description', content: `Lyrics of ${this.song.description}` },
        { name: 'twitter:image', content: '/assets/logo/music_note_128.png' }
      ]);
    } else {
      return this.metaService.addTags([
        { name: 'twitter:type', content: 'music.song' },
        { name: 'twitter:card', content: 'player' },
        { name: 'twitter:url', content: this.baseUrl + this.location.path() },
        { name: 'twitter:title', content: this.song.description },
        { name: 'twitter:description', content: `Video and lyrics of ${this.song.description}` },
        { name: 'twitter:image', content: `http://i.ytimg.com/vi/${this.song.youtubeId}/hqdefault.jpg` },
        { name: 'twitter:player', content: `https://www.youtube.com/embed/${this.song.youtubeId}` },
        { name: 'twitter:player:width', content: '480' },
        { name: 'twitter:player:height', content: '270' }
      ]);
    }
  }
  private addOpenGraph() {
    if (this.song.youtubeId === null) {
      return this.metaService.addTags([
        { name: 'og:type', content: 'music.song' },
        { name: 'og:url', content: this.baseUrl + this.location.path() },
        { name: 'og:title', content: this.song.description },
        { name: 'og:image', content: `${this.baseUrl}/assets/logo/music_note_128.png` },
        { name: 'og:description', content: `Lyrics of ${this.song.description}` }
      ]);
    } else {
      return this.metaService.addTags([
        { name: 'og:type', content: 'music.song' },
        { name: 'og:url', content: this.baseUrl + this.location.path() },
        { name: 'og:title', content: this.song.description },
        { name: 'og:image', content: `http://i.ytimg.com/vi/${this.song.youtubeId}/hqdefault.jpg` },
        { name: 'og:description', content: `Video and lyrics of ${this.song.description}` },
        { name: 'og:video', content: `https://youtube.com/watch?v=${this.song.youtubeId}` },
        { name: 'og:video:type', content: 'video/mp4' },
      ]);
    }
  }
  private addOpenGraphMusicians() {
    return this.metaService.addTags(this.song.artists.map((artist) => {
      return { name: 'og:musician', content: artist.name }
    }));
  }

  ngOnInit() {
    this.route.params.subscribe((routeParams) => {
      this.loadSong(routeParams.id).then(() => {
        this.metaTags = this.metaTags
          .concat(this.addTwitterCard())
          .concat(this.addOpenGraph())
          .concat(this.addOpenGraphMusicians());
      });
    });
  }

  ngOnDestroy() {
    this.metaTags.forEach((tag) => {
      tag.remove();
    });
    this.metaTags = [];
  }

  public deleteSong() {
    this.songService.deleteSong(this.song).then(() => {
      this.router.navigate(["song"]);
    });
  }

  @Output() addToPlaylist: EventEmitter<Song> = new EventEmitter();
  public doAddToPlaylist() {
    this.addToPlaylist.emit(this.song);
  }

  public song: Song = {
    id: 0,
    title: "",
    released: null,
    artists: [],
    media: [],
    lyrics: "",
    text: "",
    description: '',
    youtubeId: ''
  };
}
