import * as React from 'react';

export default class TicketKey extends React.Component<{ keyName: string }> {
    public render() {

        return <div style={style}>
            <h3>{this.props.keyName}</h3>
        </div>

    }
}

const style = {
    color: 'black'
};