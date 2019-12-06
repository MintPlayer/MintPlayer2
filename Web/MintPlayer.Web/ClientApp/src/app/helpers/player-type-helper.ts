import { Injectable } from '@angular/core';
import { ePlayerType } from '../enums/ePlayerType';
import { PlayerType } from '../interfaces/player-type';

@Injectable({
  providedIn: 'root'
})
export class PlayerTypeHelper {
  public getPlayerTypes() {
    var playerTypeValues = Object.values(ePlayerType);
    var playerTypeValuesSpliced = playerTypeValues.splice(playerTypeValues.length / 2);
    
    return playerTypeValuesSpliced.map<PlayerType>((value) => {
      return {
        value: <ePlayerType>value,
        description: playerTypeValues[value]
      };
    });
  }
}
