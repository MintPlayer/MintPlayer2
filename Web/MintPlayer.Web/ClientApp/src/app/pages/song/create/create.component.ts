import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
import { Song } from '../../../interfaces/song';
import { Artist } from '../../../interfaces/artist';
import { SongService } from '../../../services/song/song.service';
import { MediumType } from '../../../interfaces/medium-type';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class SongCreateComponent implements OnInit {

  constructor(private songService: SongService, private router: Router, private route: ActivatedRoute, private titleService: Title) {
		this.titleService.setTitle('Add new song');
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
    text: "",
    description: '',
    youtubeId: ''
  };
  mediumTypes: MediumType[] = [];

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

  httpHeaders: HttpHeaders = new HttpHeaders({
		'include_relations': String(true)
	});

	saveSong() {
		this.songService.createSong(this.song).subscribe((song) => {
			this.router.navigate(["song", song.id]);
		});
  }

}
