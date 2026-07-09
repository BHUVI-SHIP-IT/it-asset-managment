import { TestBed } from '@angular/core/testing';

import { Component } from './component';

describe('Component', () => {
  let service: Component;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Component);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
