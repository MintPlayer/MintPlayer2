/// <reference path="../../../../node_modules/@types/youtube/index.d.ts" />

import { Component, OnInit, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { YoutubeHelper } from '../../helpers/youtube-helper';

@Component({
  selector: 'youtube-player',
  templateUrl: './youtube-player.component.html',
  styleUrls: ['./youtube-player.component.scss']
})
export class YoutubePlayerComponent implements OnInit, AfterViewInit {

  constructor(private youtubeHelper: YoutubeHelper) {
  }

  @Input() domId: string;
  @Input() width: number;
  @Input() height: number;

  @Output() songEnded: EventEmitter<any> = new EventEmitter();

  private player: YT.Player;
  private isApiReady: boolean = false;

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.youtubeHelper.apiReady.subscribe((ready) => {
      console.log('api ready callback');
      this.isApiReady = ready;
      if (ready && !this.player) {
        console.log('Create youtube player');
        this.player = new YT.Player(this.domId, {
          width: this.width,
          height: this.height,
          events: {
            onReady: () => {
              console.log('youtube player ready');
            },
            onStateChange: (state: any) => {
              switch (state.data) {
                case YT.PlayerState.PLAYING:
                  break;
                case YT.PlayerState.PAUSED:
                  break;
                case YT.PlayerState.ENDED:
                  this.songEnded.emit();
                  break;
              }
            }
          }
        });
      }
    });
  }

  public playSong(youtubeId: string) {
    if (this.isApiReady) {
      this.player.loadVideoById(youtubeId);
    }
  }

}
