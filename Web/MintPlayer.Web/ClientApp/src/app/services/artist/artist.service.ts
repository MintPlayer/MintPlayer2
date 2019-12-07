import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Artist } from '../../interfaces/artist';

@Injectable({
  providedIn: 'root'
})
export class ArtistService {
  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public getArtists(include_relations: boolean, count: number = 20, page: number = 1) {
    return this.httpClient.get<Artist[]>(`${this.baseUrl}/web/artist`, {
      headers: {
        'include_relations': String(include_relations),
        'count': String(count),
        'page': String(page)
      }
    }).toPromise();
  }

  public getArtist(id: number, include_relations: boolean) {
    return this.httpClient.get<Artist>(`${this.baseUrl}/web/artist/${id}`, {
      headers: {
        'include_relations': String(include_relations)
      }
    }).toPromise();
  }

  public createArtist(artist: Artist) {
    return this.httpClient.post<Artist>(`${this.baseUrl}/web/artist`, { artist }).toPromise();
  }

  public updateArtist(artist: Artist) {
    return this.httpClient.put(`${this.baseUrl}/web/artist/${artist.id}`, { artist }).toPromise();
  }

  public deleteArtist(artist: Artist) {
    return this.httpClient.delete(`${this.baseUrl}/web/artist/${artist.id}`).toPromise();
  }
}
