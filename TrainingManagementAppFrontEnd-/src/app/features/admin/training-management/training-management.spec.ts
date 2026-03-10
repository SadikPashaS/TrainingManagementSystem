import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainingManagementComponent } from './training-management';

describe('TrainingManagement', () => {
  let component: TrainingManagementComponent;
  let fixture: ComponentFixture<TrainingManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TrainingManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainingManagementComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
