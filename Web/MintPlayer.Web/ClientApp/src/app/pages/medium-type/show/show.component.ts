import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { MediumType } from '../../../interfaces/medium-type';
import { ePlayerType } from '../../../enums/ePlayerType';
import { MediumTypeService } from '../../../services/medium-type/medium-type.service';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.scss']
})
export class MediumTypeShowComponent implements OnInit {
  constructor(private mediumTypeService: MediumTypeService, private router: Router, private route: ActivatedRoute, private titleService: Title) {
    var id = parseInt(this.route.snapshot.paramMap.get("id"));
    this.mediumTypeService.getMediumType(id, true).then((mediumtype) => {
      this.mediumType = mediumtype;
      this.titleService.setTitle(`Medium type: ${mediumtype.description}`);
    });
  }

  public deleteMediumType() {
    this.mediumTypeService.deleteMediumType(this.mediumType).then(() => {
      this.router.navigate(["medium-type"]);
    });
  }

  public playerTypeEnum = ePlayerType;
  public mediumType: MediumType = {
    id: 0,
    description: "",
    playerType: ePlayerType.None
  };

  ngOnInit() {
  }
}
