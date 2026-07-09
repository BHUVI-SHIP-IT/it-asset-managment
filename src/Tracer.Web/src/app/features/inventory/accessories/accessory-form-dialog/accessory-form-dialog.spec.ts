import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccessoryFormDialog } from './accessory-form-dialog';

describe('AccessoryFormDialog', () => {
  let component: AccessoryFormDialog;
  let fixture: ComponentFixture<AccessoryFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessoryFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(AccessoryFormDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
