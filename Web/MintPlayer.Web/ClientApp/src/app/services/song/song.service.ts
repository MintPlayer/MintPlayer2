import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Song } from '../../interfaces/song';

@Injectable({
  providedIn: 'root'
})
export class SongService {
  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public getSongs(include_relations: boolean, count: number = 20, page: number = 1) {
    return this.httpClient.get<Song[]>(`${this.baseUrl}/web/song`, {
      headers: {
        'include_relations': String(include_relations),
        'count': String(count),
        'page': String(page)
      }
    }).toPromise();
  }

  public getSong(id: number, include_relations: boolean) {
    return this.httpClient.get<Song>(`${this.baseUrl}/web/song/${id}`, {
      headers: {
        'include_relations': String(include_relations)
      }
    }).toPromise();
  }

  public createSong(song: Song) {
    return this.httpClient.post<Song>(`${this.baseUrl}/web/song`, { song }).toPromise();
  }

  public updateSong(song: Song) {
    return this.httpClient.put(`${this.baseUrl}/web/song/${song.id}`, { song }).toPromise();
  }

  public deleteSong(song: Song) {
    return this.httpClient.delete(`${this.baseUrl}/web/song/${song.id}`).toPromise();
  }
}
