// Link.react.test.js
import * as React from 'react';
import TicketKey from '../components/TicketKey';
import * as renderer from 'react-test-renderer';

test('Renders keyname prop', () => {
    const component = renderer.create(
        <TicketKey keyName="123" />,
    );

    let tree = component.toJSON();
    expect(tree).toMatchSnapshot();
});