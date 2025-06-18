import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MotorListComponent } from './motor-list.component';

describe('MotorListComponent', () => {
  let component: MotorListComponent;
  let fixture: ComponentFixture<MotorListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MotorListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MotorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
