import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ArtistShowComponent } from './show.component';

describe('ArtistShowComponent', () => {
  let component: ArtistShowComponent;
  let fixture: ComponentFixture<ArtistShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ArtistShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ArtistShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
