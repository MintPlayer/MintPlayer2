import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MediumTypeEditComponent } from './edit.component';

describe('MediumTypeEditComponent', () => {
  let component: MediumTypeEditComponent;
  let fixture: ComponentFixture<MediumTypeEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MediumTypeEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MediumTypeEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
