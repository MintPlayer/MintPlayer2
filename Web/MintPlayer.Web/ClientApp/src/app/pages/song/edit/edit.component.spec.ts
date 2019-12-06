import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SongEditComponent } from './edit.component';

describe('SongEditComponent', () => {
  let component: SongEditComponent;
  let fixture: ComponentFixture<SongEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SongEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SongEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
