import { Component, OnInit, Input } from '@angular/core';
import { MediumType } from '../../../interfaces/medium-type';
import { Medium } from '../../../interfaces/medium';
import { MediumTypeService } from '../../../services/medium-type/medium-type.service';

@Component({
  selector: 'media-manager',
  templateUrl: './media-manager.component.html',
  styleUrls: ['./media-manager.component.scss']
})
export class MediaManagerComponent implements OnInit {

  constructor(private mediumTypeService: MediumTypeService) {
    this.mediumTypeService.getMediumTypes(false).then((result) => {
      this.mediumTypes = result;
    });
  }

  ngOnInit() {
  }

  @Input() mediumTypes: MediumType[];
  @Input() media: Medium[];

  addMedium() {
    this.media.push({
      id: 0,
      type: this.mediumTypes.length === 0 ? null : this.mediumTypes[0],
      value: ""
    });
  }

  removeMedium(medium: Medium) {
    this.media.splice(this.media.indexOf(medium), 1);
  }

  compareMediumTypes(m1: MediumType, m2: MediumType) {
    if (m1 == null || m2 == null) {
      return false;
    } else {
      return m1.id == m2.id;
    }
  }

}
