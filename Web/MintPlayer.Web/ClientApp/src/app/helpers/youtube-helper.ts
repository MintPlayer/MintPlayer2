import { Promise } from 'q';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class YoutubeHelper {
  static scriptTag: HTMLScriptElement = null;

  public loadApi() {
    return Promise((resolve) => {
      if (YoutubeHelper.scriptTag === null) {
        if (typeof window !== 'undefined') {
          window['onYouTubeIframeAPIReady'] = resolve;

          YoutubeHelper.scriptTag = window.document.createElement('script');
          YoutubeHelper.scriptTag.src = 'https://www.youtube.com/iframe_api';

          const firstScriptTag = window.document.getElementsByTagName('script')[0];
          firstScriptTag.parentNode.insertBefore(YoutubeHelper.scriptTag, firstScriptTag);
        }
      }
    });
  }

  public unloadApi() {
    return Promise((resolve) => {
      if (YoutubeHelper.scriptTag !== null) {
        YoutubeHelper.scriptTag.remove();
        YoutubeHelper.scriptTag = null;
      }
    });
  }

  public apiReady = new BehaviorSubject<boolean>(
    (typeof window === 'undefined')
      ? false
      : window['YT'] !== undefined
  );
}
