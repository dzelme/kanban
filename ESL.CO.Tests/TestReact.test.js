import React from 'react';
import { mount, shallow } from 'enzyme';
import { expect } from 'chai';

import TicketSummary from '../ESL.CO.React/ClientApp/components/TicketSummary';

describe('Test <TicketSummary>', function () {
    it('should have a summary text to show', function () {
        const wrapper = shallow(<TicketSummary />);
        expect(wrapper.find('img')).to.have.length(1);
    });
});