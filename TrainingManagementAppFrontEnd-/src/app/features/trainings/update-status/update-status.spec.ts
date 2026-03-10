import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateStatusComponent } from './update-status';

describe('UpdateStatus', () => {
  let component: UpdateStatusComponent;
  let fixture: ComponentFixture<UpdateStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UpdateStatusComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateStatusComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
