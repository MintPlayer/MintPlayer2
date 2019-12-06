import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MediumTypeShowComponent } from './show.component';

describe('MediumTypeShowComponent', () => {
  let component: MediumTypeShowComponent;
  let fixture: ComponentFixture<MediumTypeShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MediumTypeShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MediumTypeShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
