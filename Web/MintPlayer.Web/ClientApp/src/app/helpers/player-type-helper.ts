import { Injectable } from '@angular/core';
import { ePlayerType } from '../enums/ePlayerType';
import { PlayerType } from '../interfaces/player-type';

@Injectable({
  providedIn: 'root'
})
export class PlayerTypeHelper {
  public getPlayerTypes() {
    var playerTypes = Object.keys(ePlayerType);
    var result = playerTypes.splice(playerTypes.length / 2).map<PlayerType>((key) => {
      return {
        value: playerTypes[key],
        description: key
      };
    });
    debugger;
    return result;
  }
}
