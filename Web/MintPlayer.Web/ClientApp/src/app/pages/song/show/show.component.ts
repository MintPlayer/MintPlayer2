import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { Song } from '../../../interfaces/song';
import { SongService } from '../../../services/song/song.service';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.scss']
})
export class SongShowComponent implements OnInit {
  constructor(private songService: SongService, private router: Router, private route: ActivatedRoute, private titleService: Title) {
    var id = parseInt(this.route.snapshot.paramMap.get("id"));
    this.loadSong(id);
  }

  private loadSong(id: number) {
    this.songService.getSong(id, true).subscribe(song => {
      this.song = song;
      if (song != null) {
        this.titleService.setTitle(`${song.title}: Video and lyrics`);
      }
    });
  }

  ngOnInit() {
    this.route.params.subscribe(routeParams => {
    	this.loadSong(routeParams.id);
    });
  }

  public deleteSong() {
    this.songService.deleteSong(this.song).subscribe(() => {
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
