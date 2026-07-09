import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentFormDialog } from './component-form-dialog';

describe('ComponentFormDialog', () => {
  let component: ComponentFormDialog;
  let fixture: ComponentFixture<ComponentFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComponentFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ComponentFormDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
