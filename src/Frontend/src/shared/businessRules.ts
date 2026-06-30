import type { OfferResponse } from '../api/offersApi'
import type { OrderResponse } from '../api/ordersApi'

// Shared offer and order status rules.
//
// These helpers keep the offer/order overview filters and the dashboard
// counts in sync: every place that asks "is this offer open?" or
// "is this order active?" uses the same definition, so a count on the
// dashboard can never disagree with the filter it links to.

export function getRelatedOrder(orders: OrderResponse[], offerId: string) {
  return orders.find((order) => order.offerId === offerId)
}

export function isOpenOffer(offer: OfferResponse, relatedOrder?: OrderResponse) {
  return (
    offer.status === 1 ||
    offer.status === 2 ||
    (offer.status === 3 && !relatedOrder)
  )
}

export function isArchivedOffer(
  offer: OfferResponse,
  relatedOrder?: OrderResponse
) {
  return offer.status === 4 || relatedOrder !== undefined
}

export function isActiveOrder(order: OrderResponse) {
  return order.status === 1 || order.status === 2
}

export function isDoneOrder(order: OrderResponse) {
  return order.status === 3 || order.status === 4
}
