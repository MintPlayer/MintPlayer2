import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MediumTypeCreateComponent } from './create.component';

describe('MediumTypeCreateComponent', () => {
  let component: MediumTypeCreateComponent;
  let fixture: ComponentFixture<MediumTypeCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MediumTypeCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MediumTypeCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
