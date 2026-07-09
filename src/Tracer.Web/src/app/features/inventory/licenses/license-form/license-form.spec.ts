import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseForm } from './license-form';

describe('LicenseForm', () => {
  let component: LicenseForm;
  let fixture: ComponentFixture<LicenseForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LicenseForm],
    }).compileComponents();

    fixture = TestBed.createComponent(LicenseForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
