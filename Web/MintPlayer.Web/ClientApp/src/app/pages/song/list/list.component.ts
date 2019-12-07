import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { SongService } from '../../../services/song/song.service';
import { Song } from '../../../interfaces/song';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class SongListComponent implements OnInit {
  constructor(private songService: SongService, private router: Router, private route: ActivatedRoute, private titleService: Title) {
    this.titleService.setTitle('All songs');
    this.loadSongs();
  }

  private loadSongs() {
    this.songService.getSongs(false).then((songs) => {
      this.songs = songs;
    });
  }

  ngOnInit() {
  }

  public songs: Song[] = [];
}
