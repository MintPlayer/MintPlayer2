import { Component, OnInit } from '@angular/core';
import { SongService } from '../../../services/song/song.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { Song } from '../../../interfaces/song';
import { HttpHeaders } from '@angular/common/http';
import { Artist } from '../../../interfaces/artist';
import { MediumType } from '../../../interfaces/medium-type';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class SongEditComponent implements OnInit {
  constructor(private songService: SongService, private router: Router, private route: ActivatedRoute, private titleService: Title) {
    var id = parseInt(this.route.snapshot.paramMap.get("id"));
    this.songService.getSong(id, true).subscribe(song => {
      this.song = song;
      this.titleService.setTitle(`Edit song: ${song.title}`);
      this.oldSongTitle = song.title;
    });
  }

  ngOnInit() {
  }

  oldSongTitle: string = "";
  song: Song = {
    id: 0,
    title: "",
    released: null,
    artists: [],
    media: [],
    lyrics: "",
    text: ""
  };
  mediumTypes: MediumType[] = [];

  public httpHeaders: HttpHeaders = new HttpHeaders({
    'include_relations': String(true)
  });

  public artistChanged(artist: [Artist, string]) {
    var action = artist[1]; // add, remove
    switch (action) {
      case 'add':
        this.song.artists.push(artist[0]);
        break;
      case 'remove':
        this.song.artists.splice(this.song.artists.indexOf(artist[0]), 1);
        break;
    }
  }

  public updateSong() {
    this.songService.updateSong(this.song).subscribe(() => {
      this.router.navigate(["song", this.song.id]);
    });
  }
}
