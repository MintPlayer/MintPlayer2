import { Component, OnInit, OnDestroy } from '@angular/core';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  constructor(private metaService: Meta) {
  }

  private metaTags: HTMLMetaElement[] = [];
  ngOnInit() {
    this.metaTags = this.metaService.addTags([
      { name: 'twitter:card', content: 'summary_large_image' },
      { name: 'twitter:site', content: '@mintplayer' },
      { name: 'twitter:title', content: 'Open-source music player' },
      { name: 'twitter:description', content: 'MintPlayer is an online, open-source music player.' },
      { name: 'twitter:image', content: '/assets/logo/music_note_128.png' }
    ]);
  }
  ngOnDestroy() {
    this.metaTags.forEach((tag) => {
      tag.remove();
    });
    this.metaTags = [];
  }

}
