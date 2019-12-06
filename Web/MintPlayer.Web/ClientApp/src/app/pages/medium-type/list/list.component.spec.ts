import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MediumTypeListComponent } from './list.component';

describe('MediumTypeListComponent', () => {
  let component: MediumTypeListComponent;
  let fixture: ComponentFixture<MediumTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MediumTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MediumTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
