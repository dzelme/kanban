import * as React from 'react';

export default class TicketKey extends React.Component<{ keyName: string }> {
    public render() {

        return <div>
            <h4><strong>{this.props.keyName}</strong></h4>
        </div>

    }
}